﻿/*
 * Copyright (C) 2009, Henon <meinrad.recheis@gmail.com>
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * - Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above
 *   copyright notice, this list of conditions and the following
 *   disclaimer in the documentation and/or other materials provided
 *   with the distribution.
 *
 * - Neither the name of the Git Development Community nor the
 *   names of its contributors may be used to endorse or promote
 *   products derived from this software without specific prior
 *   written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ObjectId = GitSharp.Core.ObjectId;
using CoreRef = GitSharp.Core.Ref;
using CoreCommit = GitSharp.Core.Commit;
using CoreTree = GitSharp.Core.Tree;
using CoreTag = GitSharp.Core.Tag;
using System.Diagnostics;

namespace GitSharp
{
    public abstract class AbstractObject
    {
        protected Repository _repo;
        internal ObjectId _id; // <--- the git object is lazy loaded. only a _id is required until properties are accessed.

        internal AbstractObject(Repository repo, ObjectId id)
        {
            _repo = repo;
            _id = id;
        }

        internal AbstractObject(Repository repo, string name)
        {
            _repo = repo;
            _id = _repo._internal_repo.Resolve(name);
        }

        /// <summary>
        /// The object's SHA1 hash.
        /// </summary>
        public string Hash
        {
            get
            {
                return _id.ToString();
            }
        }

        /// <summary>
        /// the object's abbreviated SHA1 hash
        /// </summary>
        public string ShortHash
        {
            get
            {
                return _id.Abbreviate(_repo._internal_repo).name();
            }
        }

        /// <summary>
        /// True if the internal object is a blob. May be used for checking if the API object type correctly represents the internal object.
        /// </summary>
        public bool IsBlob
        {
            get
            {
                return _repo._internal_repo.MapObject(_id, null) is byte[];
            }
        }

        /// <summary>
        /// True if the internal object is a blob. May be used for checking if the API object type correctly represents the internal object.
        /// </summary>
        public bool IsCommit
        {
            get
            {
                return _repo._internal_repo.MapObject(_id, null) is CoreCommit;
            }
        }

        /// <summary>
        /// True if the internal object is a blob. May be used for checking if the API object type correctly represents the internal object.
        /// </summary>
        public bool IsTag
        {
            get
            {
                return _repo._internal_repo.MapObject(_id, null) is CoreTag;
            }
        }

        /// <summary>
        /// True if the internal object is a blob. May be used for checking if the API object type correctly represents the internal object.
        /// </summary>
        public bool IsTree
        {
            get
            {
                return _repo._internal_repo.MapObject(_id, null) is CoreTree;
            }
        }

        public Repository Repository
        {
            get
            {
                return _repo;
            }
        }

#if implemented
        public Diff Diff(AbstractObject other) { }

        public ?? Grep(?? pattern) { }

        public Byte[] Content { get; }

        public long Size { get; }
#endif

        /// <summary>
        /// Internal helper function to create the right object instance for a given hash
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        internal static AbstractObject Wrap(Repository repo, ObjectId objectId)
        {
            Debug.Assert(objectId != null);
            Debug.Assert(repo != null);
            var obj = repo._internal_repo.MapObject(objectId, null);
            if (obj is CoreCommit)
                return new Commit(repo, obj as CoreCommit);
            else if (obj is CoreTag)
                return new Tag(repo, obj as CoreTag);
            else if (obj is CoreTree)
                return new Tree(repo, obj as CoreTree);
            else if (obj is byte[])
                return new Blob(repo, objectId, obj as byte[]);
            else
            {
                //Debug.Assert(false, "What kind of object do we have here?");
                return null;
            }
        }

        #region Equality overrides

        /// <summary>
        /// Overriding equals to reflect that different AbstractObject instances with the same hash are in fact equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is AbstractObject)
                return _id == (obj as AbstractObject)._id;
            else
                return false;
        }

        public static bool operator ==(AbstractObject self, object other)
        {
            return Equals(self, other);
        }

        public static bool operator !=(AbstractObject self, object other)
        {
            return !(self == other);
        }

        public override int GetHashCode()
        {
            if (_id != null)
                return _id.GetHashCode();
            return base.GetHashCode();
        }

        #endregion

    }
}
