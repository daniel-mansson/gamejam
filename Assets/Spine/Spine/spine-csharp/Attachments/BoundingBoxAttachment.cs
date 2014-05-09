/******************************************************************************
 * Spine Runtimes Software License
 * Version 2.1
 * 
 * Copyright (c) 2013, Esoteric Software
 * All rights reserved.
 * 
 * You are granted a perpetual, non-exclusive, non-sublicensable and
 * non-transferable license to install, execute and perform the Spine Runtimes
 * Software (the "Software") solely for internal use. Without the written
 * permission of Esoteric Software (typically granted by licensing Spine), you
 * may not (a) modify, translate, adapt or otherwise create derivative works,
 * improvements of the Software or develop new applications using the Software
 * or (b) remove, delete, alter or obscure any trademarks or any copyright,
 * trademark, patent or other intellectual property or proprietary rights
 * notices on or in the Software, including any copy thereof. Redistributions
 * in binary or source form must include this license and terms.
 * 
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;

namespace Spine {
	/// <summary>Attachment that has a polygon for bounds checking.</summary>
	public class BoundingBoxAttachment : Attachment {
		internal float[] vertices;

		public float[] Vertices { get { return vertices; } set { vertices = value; } }

		public BoundingBoxAttachment (string name)
			: base(name) {
		}

		/// <param name="worldVertices">Must have at least the same length as this attachment's vertices.</param>
		public void ComputeWorldVertices (float x, float y, Bone bone, float[] worldVertices) {
			x += bone.worldX;
			y += bone.worldY;
			float m00 = bone.m00;
			float m01 = bone.m01;
			float m10 = bone.m10;
			float m11 = bone.m11;
			float[] vertices = this.vertices;
			for (int i = 0, n = vertices.Length; i < n; i += 2) {
				float px = vertices[i];
				float py = vertices[i + 1];
				worldVertices[i] = px * m00 + py * m01 + x;
				worldVertices[i + 1] = px * m10 + py * m11 + y;
			}
		}
	}
}
